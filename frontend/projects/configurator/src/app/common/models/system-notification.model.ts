import {SystemNotificationType} from '../enums/system-notification-type.enum';

export interface SystemNotificationModel {
  type: SystemNotificationType;
  parameters: any;
}
