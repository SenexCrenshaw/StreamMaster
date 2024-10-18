import { SyntheticEvent } from 'react';

export interface ChildButtonProperties {
  buttonClassName?: string;
  buttonDisabled?: boolean;
  iconFilled?: boolean;
  isLeft?: boolean;
  label?: string;
  onClick?: (e: SyntheticEvent) => void;
  style?: React.CSSProperties;
  tooltip?: string;
  outlined?: boolean;
}
