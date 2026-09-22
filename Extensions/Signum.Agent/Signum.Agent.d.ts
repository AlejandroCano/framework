import { MessageKey, Type, EnumType } from '../../Signum/React/Reflection';
import * as Entities from '../../Signum/React/Signum.Entities';
import * as Basics from '../../Signum/React/Signum.Basics';
import * as Operations from '../../Signum/React/Signum.Operations';
import * as Authorization from '../Signum.Authorization/Signum.Authorization';
export interface ToolCallEmbedded {
    _response?: ChatMessageEntity;
}
export declare namespace AgentOperation {
    const Save: Operations.ExecuteSymbol<AgentSymbol>;
}
export declare const AgentSymbol: Type<AgentSymbol>;
export interface AgentSymbol extends Basics.SemiSymbol {
    Type: "Agent";
    skillCustomization: Entities.Lite<SkillCustomizationEntity> | null;
}
export declare const ChatbotConfigurationEmbedded: Type<ChatbotConfigurationEmbedded>;
export interface ChatbotConfigurationEmbedded extends Entities.EmbeddedEntity {
    Type: "ChatbotConfigurationEmbedded";
    openAIAPIKey: string | null;
    anthropicAPIKey: string | null;
    geminiAPIKey: string | null;
    mistralAPIKey: string | null;
    githubModelsToken: string | null;
    deepSeekAPIKey: string | null;
    ollamaUrl: string | null;
}
export declare const ChatbotLanguageModelEntity: Type<ChatbotLanguageModelEntity>;
export interface ChatbotLanguageModelEntity extends Entities.Entity {
    Type: "ChatbotLanguageModel";
    provider: LanguageModelProviderSymbol;
    model: string;
    temperature: number | null;
    maxTokens: number | null;
    isDefault: boolean;
    pricePerInputToken: number | null;
    pricePerOutputToken: number | null;
    pricePerCachedInputToken: number | null;
    pricePerReasoningOutputToken: number | null;
}
export declare namespace ChatbotLanguageModelOperation {
    const Save: Operations.ExecuteSymbol<ChatbotLanguageModelEntity>;
    const MakeDefault: Operations.ExecuteSymbol<ChatbotLanguageModelEntity>;
    const Delete: Operations.DeleteSymbol<ChatbotLanguageModelEntity>;
}
export declare namespace ChatbotMessage {
    const OpenSession: MessageKey;
    const NewSession: MessageKey;
    const Send: MessageKey;
    const TypeAMessage: MessageKey;
    const InitialInstruction: MessageKey;
    const ShowSystem: MessageKey;
    const UnableToChangeModelOrProviderOnceUsed: MessageKey;
    const WhatWentWrong: MessageKey;
    const ProvideFeedback: MessageKey;
    const Price: MessageKey;
    const TotalPrice: MessageKey;
    const AnswerAbovePlease: MessageKey;
    const MessageMustBeTheLastToDelete: MessageKey;
    const SessionInterruptedDoYouWantToRecover: MessageKey;
    const Recover: MessageKey;
    const Reasoning: MessageKey;
}
export declare namespace ChatbotPermission {
    const UseChatbot: Basics.PermissionSymbol;
}
export declare const ChatbotUICommand: EnumType<ChatbotUICommand>;
export type ChatbotUICommand = "System" | "SessionId" | "SessionTitle" | "QuestionId" | "MessageId" | "AssistantStarted" | "AssistantAnswer" | "AssistantReasoning" | "AssistantTool" | "AssistantUITool" | "Tool" | "Exception";
export declare const ChatMessageEntity: Type<ChatMessageEntity>;
export interface ChatMessageEntity extends Entities.Entity {
    Type: "ChatMessage";
    chatSession: Entities.Lite<ChatSessionEntity>;
    creationDate: string;
    role: ChatMessageRole;
    content: string | null;
    reasoningContent: string | null;
    toolCalls: Entities.MList<ToolCallEmbedded>;
    toolCallID: string | null;
    toolID: string | null;
    exception: Entities.Lite<Basics.ExceptionEntity> | null;
    languageModel: Entities.Lite<ChatbotLanguageModelEntity> | null;
    inputTokens: number | null;
    cachedInputTokens: number | null;
    outputTokens: number | null;
    reasoningOutputTokens: number | null;
    duration: string | null;
    userFeedback: UserFeedback | null;
    userFeedbackMessage: string | null;
}
export declare namespace ChatMessageOperation {
    const Delete: Operations.DeleteSymbol<ChatMessageEntity>;
}
export declare const ChatMessageRole: EnumType<ChatMessageRole>;
export type ChatMessageRole = "System" | "User" | "Assistant" | "Tool";
export declare const ChatSessionEntity: Type<ChatSessionEntity>;
export interface ChatSessionEntity extends Entities.Entity {
    Type: "ChatSession";
    title: string | null;
    languageModel: Entities.Lite<ChatbotLanguageModelEntity>;
    user: Entities.Lite<Authorization.UserEntity>;
    startDate: string;
    totalInputTokens: number | null;
    totalOutputTokens: number | null;
    totalCachedInputTokens: number | null;
    totalReasoningOutputTokens: number | null;
    totalToolCalls: number;
}
export declare namespace ChatSessionOperation {
    const Delete: Operations.DeleteSymbol<ChatSessionEntity>;
}
export declare namespace DefaultAgent {
    const Chatbot: AgentSymbol;
    const QuestionSummarizer: AgentSymbol;
    const ConversationSumarizer: AgentSymbol;
}
export declare const EmbeddingsLanguageModelEntity: Type<EmbeddingsLanguageModelEntity>;
export interface EmbeddingsLanguageModelEntity extends Entities.Entity {
    Type: "EmbeddingsLanguageModel";
    provider: LanguageModelProviderSymbol;
    model: string;
    dimensions: number | null;
    isDefault: boolean;
}
export declare namespace EmbeddingsLanguageModelOperation {
    const Save: Operations.ExecuteSymbol<EmbeddingsLanguageModelEntity>;
    const MakeDefault: Operations.ExecuteSymbol<EmbeddingsLanguageModelEntity>;
    const Delete: Operations.DeleteSymbol<EmbeddingsLanguageModelEntity>;
}
export declare namespace LanguageModelProviders {
    const OpenAI: LanguageModelProviderSymbol;
    const Gemini: LanguageModelProviderSymbol;
    const Anthropic: LanguageModelProviderSymbol;
    const Mistral: LanguageModelProviderSymbol;
    const GithubModels: LanguageModelProviderSymbol;
    const Ollama: LanguageModelProviderSymbol;
    const DeepSeek: LanguageModelProviderSymbol;
}
export declare const LanguageModelProviderSymbol: Type<LanguageModelProviderSymbol>;
export interface LanguageModelProviderSymbol extends Basics.Symbol {
    Type: "LanguageModelProvider";
}
export declare const SkillActivation: EnumType<SkillActivation>;
export type SkillActivation = "Eager" | "Lazy";
export declare const SkillCodeEntity: Type<SkillCodeEntity>;
export interface SkillCodeEntity extends Entities.Entity {
    Type: "SkillCode";
    className: string;
}
export declare const SkillCustomizationEntity: Type<SkillCustomizationEntity>;
export interface SkillCustomizationEntity extends Entities.Entity {
    Type: "SkillCustomization";
    skillCode: SkillCodeEntity;
    shortDescription: string | null;
    instructions: string | null;
    properties: Entities.MList<SkillPropertyEmbedded>;
    subSkills: Entities.MList<SubSkillEmbedded>;
}
export declare namespace SkillCustomizationOperation {
    const Save: Operations.ExecuteSymbol<SkillCustomizationEntity>;
    const Delete: Operations.DeleteSymbol<SkillCustomizationEntity>;
    const CreateFromAgent: Operations.ConstructSymbol_From<SkillCustomizationEntity, AgentSymbol>;
}
export declare const SkillPropertyEmbedded: Type<SkillPropertyEmbedded>;
export interface SkillPropertyEmbedded extends Entities.EmbeddedEntity {
    Type: "SkillPropertyEmbedded";
    propertyName: string;
    value: string | null;
}
export declare const SubSkillEmbedded: Type<SubSkillEmbedded>;
export interface SubSkillEmbedded extends Entities.EmbeddedEntity {
    Type: "SubSkillEmbedded";
    skill: Entities.Entity;
    activation: SkillActivation;
}
export declare const ToolCallEmbedded: Type<ToolCallEmbedded>;
export interface ToolCallEmbedded extends Entities.EmbeddedEntity {
    Type: "ToolCallEmbedded";
    callId: string;
    toolId: string;
    arguments: string;
    isUITool: boolean;
}
export declare const UserFeedback: EnumType<UserFeedback>;
export type UserFeedback = "Positive" | "Negative";
//# sourceMappingURL=Signum.Agent.d.ts.map