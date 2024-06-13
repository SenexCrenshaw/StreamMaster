import { ReactNode, SyntheticEvent } from 'react';
import { SMModalProperties } from './SMModalProperties';

export interface SMButtonProperties extends SMModalProperties {
  readonly buttonClassName?: string;
  readonly buttonDarkBackground?: boolean;
  readonly buttonDisabled?: boolean;
  readonly buttonLabel?: string;
  readonly buttonLarge?: boolean;
  readonly buttonContent?: ReactNode;
  readonly color?: string;
  readonly hollow?: boolean;
  readonly icon?: string;
  readonly iconFilled?: boolean;
  readonly isLoading?: boolean;
  readonly label?: string;
  readonly large?: boolean;
  readonly outlined?: boolean | undefined;
  readonly rounded?: boolean;
  readonly tooltip?: string;
  readonly isLeft?: boolean;
  readonly onClick?: (e: SyntheticEvent) => void;
  getReferenceProps?: () => any;
  readonly refs?: { setReference: React.Ref<any> };
}
