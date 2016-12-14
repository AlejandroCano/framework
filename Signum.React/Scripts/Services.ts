﻿/// <reference path="../typings/whatwg-fetch/whatwg-fetch.d.ts" />
import { ModelState } from './Signum.Entities'
import { Dic } from './Globals'
import { GraphExplorer } from './Reflection'

var fetchWithAbortModule = require('./fetchWithAbort') as { fetch: typeof fetch };

export interface AjaxOptions {
    url: string;
    avoidNotifyPendingRequests?: boolean;
    avoidThrowError?: boolean;
    avoidGraphExplorer?: boolean;
    avoidAuthToken?: boolean;

    
    headers?: { [index: string]: string };
    mode?: string | RequestMode;
    credentials?: string | RequestCredentials;
    cache?: string | RequestCache;
    abortController?: { abort?: () => void };
}


export function baseUrl(options: AjaxOptions): string {
    const baseUrl = window.__baseUrl;

    if (options.url.startsWith("~/"))
        return baseUrl + options.url.after("~/");

    return options.url;
}

export function ajaxGet<T>(options: AjaxOptions): Promise<T> {
    return ajaxGetRaw(options)
        .then(a=> a.status == 204 ? undefined as any : a.json<T>());
}

export function ajaxGetRaw(options: AjaxOptions) : Promise<Response> {
    return wrapRequest(options, () =>
        fetchWithAbortModule.fetch(baseUrl(options), {
            method: "GET",
            headers: Dic.extend({
                'Accept': 'application/json',
            }, options.headers),
            mode: options.mode,
            credentials: options.credentials || "same-origin",
            cache: options.cache,
            abortController: options.abortController
        } as RequestInit));
}

export function ajaxPost<T>(options: AjaxOptions, data: any): Promise<T> {
    return ajaxPostRaw(options, data)
        .then(a=> a.status == 204 ? undefined as any : a.json<T>());
}


export function ajaxPostRaw(options: AjaxOptions, data: any): Promise<Response> {
    if (!options.avoidGraphExplorer) {
        GraphExplorer.propagateAll(data);
    }
    
    return wrapRequest(options, () =>
        fetchWithAbortModule.fetch(baseUrl(options), {
            method: "POST",
            credentials: options.credentials || "same-origin",
            headers: Dic.extend({
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            }, options.headers),
            mode: options.mode,
            cache: options.cache,
            body: JSON.stringify(data),
            abortController: options.abortController
        } as RequestInit));
}





export function wrapRequest(options: AjaxOptions, makeCall: () => Promise<Response>): Promise<Response>
{
    if (!options.avoidThrowError) {
        const call = makeCall;
        makeCall = () => ThrowErrorFilter.throwError(call);
    }

    if (!options.avoidAuthToken && AuthTokenFilter.addAuthToken) {
        let call = makeCall;
        makeCall = () => AuthTokenFilter.addAuthToken(options, call);
    }

    if (!options.avoidNotifyPendingRequests) {
        let call = makeCall;
        makeCall = () => NotifyPendingFilter.onPendingRequest(call);
    }
    
    const promise = makeCall();

    if (!(promise as any).__proto__.done)
        (promise as any).__proto__.done = Promise.prototype.done;

    return promise;

}

export module AuthTokenFilter {
    export let addAuthToken: (options: AjaxOptions, makeCall: () => Promise<Response>) => Promise<Response>;
}


export module NotifyPendingFilter {
    export let notifyPendingRequests: (pendingRequests: number) => void = () => { };
    let pendingRequests: number = 0;
    export function onPendingRequest(makeCall: () => Promise<Response>): Promise<Response> {

        notifyPendingRequests(++pendingRequests);

        return makeCall().then(
            resp => { notifyPendingRequests(--pendingRequests); return resp; },
            error => { notifyPendingRequests(--pendingRequests); throw error; });
    }
}

export module ThrowErrorFilter { 

    export function throwError(makeCall: () => Promise<Response>): Promise<Response> {

        return makeCall().then(response => {
            if (response.status >= 200 && response.status < 300) {
                return response;
            } else {
                return response.json().then((json: WebApiHttpError) => {
                    if (json.ModelState)
                        throw new ValidationError(response.statusText, json);
                    else if (json.Message)
                        throw new ServiceError(response.statusText, response.status, json);
                }) as any;
            }
        });
    }
}

let a = document.createElement("a");
document.body.appendChild(a);
a.style.display = "none";


export function saveFile(response: Response) {
    let fileName = "file.dat";
    let match = /attachment; filename=(.+)/.exec(response.headers.get("Content-Disposition"));
    if (match)
        fileName = match[1].trimEnd("\"").trimStart("\"");

    response.blob().then(blob => {
        saveFileBlob(blob, fileName);
    });
}

export function saveFileBlob(blob: Blob, fileName: string) {
    if (window.navigator.msSaveBlob)
        window.navigator.msSaveBlob(blob, fileName);
    else {
        const url = window.URL.createObjectURL(blob);
        a.href = url;

        (a as any).download = fileName;

        a.click();

        setTimeout(() => window.URL.revokeObjectURL(url), 500);
    }
}

export function b64toBlob(b64Data: string, contentType: string = "", sliceSize = 512) {
    contentType = contentType || '';
    sliceSize = sliceSize || 512;

    var byteCharacters = atob(b64Data);
    var byteArrays: Uint8Array[] = [];

    for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
        var slice = byteCharacters.slice(offset, offset + sliceSize);

        var byteNumbers = new Array(slice.length);
        for (var i = 0; i < slice.length; i++) {
            byteNumbers[i] = slice.charCodeAt(i);
        }

        var byteArray = new Uint8Array(byteNumbers);

        byteArrays.push(byteArray);
    }

    var blob = new Blob(byteArrays, { type: contentType });
    return blob;
}

export class ServiceError {
    constructor(
        public statusText: string,
        public status: number,
        public httpError: WebApiHttpError) {
    }

    get defaultIcon() {
        switch (this.httpError.ExceptionType) {
            case "UnauthorizedAccessException": return "glyphicon-lock";
            case "EntityNotFoundException": return "glyphicon-trash";
            case "UniqueKeyException": return "glyphicon-duplicate";
            default: return "glyphicon-alert";
        }
    }

    toString() {
        return this.httpError.Message;
    }
}

export interface WebApiHttpError {
    Message: string;
    ModelState?: { [member: string]: string }
    ExceptionMessage?: string;
    ExceptionType: string;
    StackTrace?: string;
    MessageDetail?: string;
    ExceptionID?: string;
}

export class ValidationError  {
    modelState: ModelState;
    message: string;

    constructor(public statusText: string, json: WebApiHttpError) {
        this.message = json.Message || "";
        this.modelState = json.ModelState!;
    }

    toString() {
        return this.statusText + "\r\n" + this.message;
    }
}

//localStorage: Domain+Browser
//sessionStorage: Browser tab

var _appName: string = "";

export function setAppNameAndRequestSessionStorage(appName: string) {
    _appName = appName;
    if (!sessionStorage.length) {
        localStorage.setItem('requestSessionStorage' + _appName, new Date().toString());
        localStorage.removeItem('requestSessionStorage' + _appName);
    }
}

//http://blog.guya.net/2015/06/12/sharing-sessionstorage-between-tabs-for-secure-multi-tab-authentication/
//To share session storage between tabs
window.addEventListener("storage", se => {

    if (se.key == 'requestSessionStorage' + _appName) {
        // Some tab asked for the sessionStorage -> send it

        localStorage.setItem('responseSessionStorage' + _appName, JSON.stringify(sessionStorage));
        localStorage.removeItem('responseSessionStorage' + _appName);

    } else if (se.key == ('responseSessionStorage' + _appName) && !sessionStorage.length) {
        // sessionStorage is empty -> fill it

        if (se.newValue) {
            const data = JSON.parse(se.newValue);

            for (let key in data) {
                sessionStorage.setItem(key, data[key]);
            }
        }
    }
});


export function makeAbortable<T>(makeCall: (abortController: FetchAbortController) => Promise<T>): () => Promise<T | undefined> {

    let requestIndex = 0;
    let responseIndex = 0;
    let abortController: FetchAbortController | undefined = undefined;

    return () => {

        if (abortController) {
            if (abortController.abort) {
                abortController.abort!();
            }
            abortController = undefined;
        }

        requestIndex++;

        var myIndex = requestIndex;

        abortController = {};

        return makeCall(abortController).then(result => {

            if (myIndex <= responseIndex) //request is too old
                return undefined;

            abortController = undefined;
            responseIndex = myIndex;
            return result;
        }, (error: TypeError) => {
            if (error.message == "Aborted request")
                return undefined;

            throw error
        });
    }
}