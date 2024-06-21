import { ReactNode } from 'react';

export interface SMCardProperties {
  readonly center?: React.ReactNode;
  readonly darkBackGround?: boolean;
  readonly hasCloseButton?: boolean;
  readonly header?: ReactNode;
  readonly info?: string;
  readonly noBorderChildren?: boolean;
  readonly openPanel?: (open: boolean) => void;
  readonly simple?: boolean;
  readonly simpleChildren?: boolean;
  readonly title?: string | undefined;
}
