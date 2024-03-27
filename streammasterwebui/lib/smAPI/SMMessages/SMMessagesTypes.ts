import { SMMessage } from '@lib/apiDefs';
export interface SendSMErrorRequest {
  detail?: string;
  summary?: string;
 }

export interface SendSMInfoRequest {
  detail?: string;
  summary?: string;
 }

export interface SendSMMessageRequest {
  message?: SMMessage;
 }

export interface SendSMWarnRequest {
  detail?: string;
  summary?: string;
 }

export interface SendSuccessRequest {
  detail?: string;
  summary?: string;
 }

