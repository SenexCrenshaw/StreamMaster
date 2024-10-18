import { ReactNode } from 'react';

export interface SMCardProperties {
  onOkClick?: () => void;
  onFullScreenToggle?: () => void;
  readonly okButtonDisabled?: boolean;
  readonly closeButtonDisabled?: boolean;
  onCloseClick?: () => void;
  // readonly answer?: boolean;
  readonly center?: React.ReactNode;
  readonly darkBackGround?: boolean;
  readonly noCloseButton?: boolean;
  readonly closeToolTip?: string;
  readonly okToolTip?: string;
  readonly header?: ReactNode;
  readonly info?: string;
  readonly noBorderChildren?: boolean;
  readonly openPanel?: (open: boolean) => void;
  readonly simple?: boolean;
  readonly simpleChildren?: boolean;
  readonly title?: string | React.ReactNode | undefined;
  // onAnswered?(): void;
}
