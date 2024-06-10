import { ReactNode } from 'react';
import { SMModalProperties } from './SMModalProperties';

export interface SMButtonProperties extends SMModalProperties {
  readonly buttonClassName?: string;
  readonly buttonDarkBackground?: boolean;
  readonly buttonDisabled?: boolean;
  readonly buttonLabel?: string;
  readonly buttonLarge?: boolean;
  readonly buttonTemplate?: ReactNode;
  readonly icon?: string;
  readonly iconFilled?: boolean;
  readonly tooltip?: string;
  readonly isLoading?: boolean;
  readonly label?: string;
}
