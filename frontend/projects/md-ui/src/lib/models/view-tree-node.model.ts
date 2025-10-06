export interface ViewTreeNode {
  nodeId: string;
  componentType: string;
  properties: { [key: string]: any };
  children: ViewTreeNode[];
  events: string[];
}

export interface EdgeInsets {
  top: number;
  right: number;
  bottom: number;
  left: number;
}

export interface ViewTreeMessage {
  sessionId: string;
  viewTree: ViewTreeNode;
  rootViewId: string;
}

export interface ViewUpdateMessage {
  sessionId: string;
  viewId: string;
  updatedNode: ViewTreeNode;
}

export interface UiEventMessage {
  sessionId: string;
  viewId: string;
  eventName: string;
  parameters?: { [key: string]: any };
}

export interface UiErrorMessage {
  sessionId: string;
  message: string;
  viewId?: string;
}
