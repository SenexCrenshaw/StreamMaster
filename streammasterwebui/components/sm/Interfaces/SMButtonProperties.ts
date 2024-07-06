import { ReactNode, SyntheticEvent } from 'react';
import { SMModalProperties } from './SMModalProperties';

export interface SMButtonProperties extends SMModalProperties {
  readonly buttonClassName?: string;
  readonly buttonTemplate?: ReactNode;
  readonly buttonDarkBackground?: boolean;
  readonly buttonDisabled?: boolean;
  readonly buttonIsLoading?: boolean;
  readonly buttonLabel?: string;
  readonly buttonLarge?: boolean;
  readonly buttonLargeImage?: boolean;
  readonly color?: string;
  readonly menu?: boolean;
  readonly icon?: string;
  readonly iconFilled?: boolean;
  readonly isLeft?: boolean;
  readonly label?: string;
  readonly noHover?: boolean;
  readonly outlined?: boolean | undefined;
  readonly rounded?: boolean;
  readonly tooltip?: string;
  readonly onClick?: (e: SyntheticEvent) => void;
  getReferenceProps?: () => any;
  readonly refs?: { setReference: React.Ref<any> };
}
