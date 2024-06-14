import { Placement } from '@floating-ui/react';
import { SMButtonProperties } from './SMButtonProperties';
import { SMCardProperties } from './SMCardProperties';
import { SMModalProperties } from './SMModalProperties';

export interface SMOverlayProperties extends SMButtonProperties, SMCardProperties, SMModalProperties {
  onCloseClick?: () => void;
  readonly answer?: boolean;
  readonly autoPlacement?: boolean;
  readonly className?: string;
  readonly closeOnLostFocus?: boolean;
  readonly contentWidthSize?: string;
  readonly isOverLayLoading?: boolean;
  readonly info?: string;
  readonly placement?: Placement;
  readonly showClose?: boolean;
  readonly zIndex?: number;
  onAnswered?(): void;
}
