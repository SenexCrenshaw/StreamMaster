import { Placement } from '@floating-ui/react';
import { ReactNode } from 'react';
import { SMButtonProperties } from './SMButtonProperties';
import { SMCardProperties } from './SMCardProperties';
import { SMModalProperties } from './SMModalProperties';

export interface SMOverlayProperties extends SMButtonProperties, SMCardProperties, SMModalProperties {
  readonly answer?: boolean;
  readonly autoPlacement?: boolean;
  readonly className?: string;
  readonly closeOnLostFocus?: boolean;
  readonly contentWidthSize?: string;
  readonly header?: ReactNode;
  readonly hasCloseButton?: boolean;
  readonly info?: string;
  readonly placement?: Placement;
  readonly showClose?: boolean;
  readonly zIndex?: number;
  onAnswered?(): void;
}
