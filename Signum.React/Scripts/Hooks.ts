import * as React from 'react'
import { FindOptions, ResultTable } from './Search';
import * as Finder from './Finder';
import * as Navigator from './Navigator';
import { Entity, Lite, liteKey, isEntity } from './Signum.Entities';
import { Type, QueryTokenString } from './Reflection';

export function useForceUpdate(): () => void {
  var [object, setObject] = React.useState({});
  return () => {
    setObject({});
  }
}

export function useUpdatedRef<T>(newValue: T): React.MutableRefObject<T> {
  const ref = React.useRef(newValue);
  ref.current = newValue;
  return ref;
}

export function useForceUpdatePromise(): () => Promise<void> {
  var [object, setObject] = useStateWithPromise({});
  return () => setObject({}) as Promise<any>;
}

export function useInterval<T>(interval: number | undefined | null, initialState: T, newState: (oldState: T) => T) {
  const [val, setVal] = React.useState(initialState);

  React.useEffect(() => {
    var insideVal = val;
    if (interval) {
      var handler = setInterval(() => {
        setVal(insideVal = newState(insideVal));
      }, interval);
      return () => clearInterval(handler);
    }
  }, [interval]);

  return val;
}

export function usePrevious<T>(value: T): T | undefined {
  var ref = React.useRef<T | undefined>();
  React.useEffect(() => {
    ref.current = value;
  }, [value]);

  return ref.current;
}

export function useExpand() {
  React.useEffect(() => {
    const wasExpanded = Navigator.Expander.setExpanded(true);
    return () => { Navigator.Expander.setExpanded(wasExpanded); }
  }, []);

}


interface Size {
  width: number;
  height: number;
}

export function useSize<T extends HTMLElement = HTMLDivElement>(): { size: Size | undefined, setContainer: (element: T | null) => void } {
  const [size, setSize] = React.useState<Size | undefined>();
  const divElement = React.useRef<T | null>(null);
  function setNewSize() {
    const rect = divElement.current!.getBoundingClientRect();
    if (size == null || size.width != rect.width || size.height != rect.height)
      setSize({ width: rect.width, height: rect.height });
  }

  function setContainer(div: T | null) {
    if (divElement.current = div) {
      setNewSize();
    }
  }
  const handler = React.useRef<number | null>(null);
  React.useEffect(() => {
    function onResize() {
      if (handler.current != null)
        clearTimeout(handler.current);

      handler.current = setTimeout(() => {
        if (divElement.current) {
          setNewSize()
        }
      }, 300);
    }

    window.addEventListener('resize', onResize);

    return () => {
      if (handler.current)
        clearTimeout(handler.current);

      window.removeEventListener("resize", onResize);
    };
  }, []);

  return { size, setContainer };
}

interface APIHookOptions {
  avoidReset?: boolean;
}

export function useStateWithPromise<T>(defaultValue: T): [T, (newValue: React.SetStateAction<T>) => Promise<T>] {
  const [state, setState] = React.useState({ value: defaultValue, resolve: (val: T) => { } });

  React.useEffect(() => state.resolve(state.value), [state]);

  return [
    state.value,
    updaterOrValue => new Promise(resolve => {
      setState(prevState => {
        let nextVal = typeof updaterOrValue == "function" ? (updaterOrValue as ((val: T) => T))(prevState.value) : updaterOrValue;
        return {
          value: nextVal,
          resolve: resolve
        };
      })
    })
  ]
}

export function useTitle(title: string, deps?: readonly any[]) {
  React.useEffect(() => {
    Navigator.setTitle(title);
    return () => Navigator.setTitle();
  }, deps);
}

export function useAPIWithReload<T>(makeCall: (signal: AbortSignal, oldData: T | undefined) => Promise<T>, deps: ReadonlyArray<any>, options?: APIHookOptions): [T | undefined, () => void] {
  const [obj, setObj] = React.useState({});
  const value = useAPI<T>(makeCall, [...(deps || []), obj], options);
  return [value, () => setObj({})];
}



export function useAPI<T>(makeCall: (signal: AbortSignal, oldData: T | undefined) => Promise<T>, deps: ReadonlyArray<any>, options?: APIHookOptions): T | undefined {

  const [data, setData] = React.useState<{ deps: ReadonlyArray<any>; result: T } | undefined>(undefined);

  React.useEffect(() => {
    var abortController = new AbortController();

    makeCall(abortController.signal, data && data.result)
      .then(result => !abortController.signal.aborted && setData({ result, deps }))
      .done();

    return () => {
      abortController.abort();
    }
  }, deps);

  if (!(options && options.avoidReset)) {
    if (data && !areEqual(data.deps, deps))
      return undefined;
  }

  return data && data.result;
}

function areEqual(depsA: ReadonlyArray<any>, depsB: ReadonlyArray<any>) {

  if (depsA.length !== depsB.length)
    return false;

  for (var i = 0; i < depsA.length; i++) {
    if (depsA[i] !== depsB[i])
      return false;
  }

  return true;
}

export function useMounted() {
  const mounted = React.useRef<boolean>(false);
  React.useEffect(() => {
    return () => { mounted.current = false; };
  }, []);
  return mounted;
}

export function useThrottle<T>(value: T, limit: number): T {
  const [throttledValue, setThrottledValue] = React.useState(value);

  const mounted = React.useRef(true);
  const lastRequested = React.useRef<(undefined | { value: T })>(undefined);
  React.useEffect(
    () => {
      if (lastRequested.current) {
        lastRequested.current.value = value;
      } else {
        lastRequested.current = { value };
        const handler = setTimeout(function () {
          if (mounted.current) {
            setThrottledValue(lastRequested.current!.value);
            lastRequested.current = undefined;

            clearTimeout(handler);
          }
        }, limit);
      }
    },
    [value, limit]
  );

  React.useEffect(() => {
    return () => { mounted.current = false; }
  }, []);

  return throttledValue;
};

export function useQuery(fo: FindOptions | null, additionalDeps?: any[], options?: APIHookOptions): ResultTable | undefined | null {
  return useAPI(signal =>
    fo == null ? Promise.resolve<ResultTable | null>(null) :
      Finder.getQueryDescription(fo.queryName)
        .then(qd => Finder.parseFindOptions(fo!, qd, false))
        .then(fop => Finder.API.executeQuery(Finder.getQueryRequest(fop), signal)),
    [fo && Finder.findOptionsPath(fo), ...(additionalDeps || [])],
    options);
}

export function useInDB<R>(entity: Entity | Lite<Entity> | null, token: QueryTokenString<R> | string, additionalDeps?: any[], options?: APIHookOptions): Finder.AddToLite<R> | null | undefined {
  var resultTable = useQuery(entity == null ? null : {
    queryName: isEntity(entity) ? entity.Type : entity.EntityType,
    filterOptions: [{ token: "Entity", value: entity }],
    pagination: { mode: "Firsts", elementsPerPage: 1 },
    columnOptions: [{ token: token }],
    columnOptionsMode: "Replace",
  }, additionalDeps, options);

  if (entity == null)
    return null;

  if (resultTable == null)
    return undefined;

  return resultTable.rows[0] && resultTable.rows[0].columns[0] || null;
}

export function useFetchAndForget<T extends Entity>(lite: Lite<T> | null | undefined): T | null | undefined {
  return useAPI(signal =>
    lite == null ? Promise.resolve<T | null | undefined>(lite) :
      Navigator.API.fetchAndForget(lite),
    [lite && liteKey(lite)]);
}


export function useFetchAndRemember<T extends Entity>(lite: Lite<T> | null, onLoaded?: () => void): T | null | undefined {

  const forceUpdate = useForceUpdate();
  React.useEffect(() => {
    if (lite && !lite.entity)
      Navigator.API.fetchAndRemember(lite)
        .then(() => {
          onLoaded && onLoaded();
          forceUpdate();
        })
        .done();
  }, [lite && liteKey(lite)]);


  if (lite == null)
    return null;

  if (lite.entity == null)
    return undefined;

  return lite.entity;
}

export function useFetchAll<T extends Entity>(type: Type<T>): T[] | undefined {
  return useAPI(signal => Navigator.API.fetchAll(type), []);
}