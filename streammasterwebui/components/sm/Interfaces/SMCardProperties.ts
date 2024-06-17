import { ReactNode } from 'react';

export interface SMCardProperties {
  readonly center?: React.ReactNode;
  readonly darkBackGround?: boolean;
  readonly header?: ReactNode;
  readonly info?: string;
  readonly hasCloseButton?: boolean;
  readonly openPanel?: (open: boolean) => void;
  readonly title?: string | undefined;

  readonly simple?: boolean;
  readonly simpleChildren?: boolean;
}
