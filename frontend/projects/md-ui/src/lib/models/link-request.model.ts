export interface LinkRequestMessage {
  requestId: string;
  sessionId: string;
  url: string;
}

export interface LinkResponseMessage {
  requestId: string;
  approved: boolean;
}
