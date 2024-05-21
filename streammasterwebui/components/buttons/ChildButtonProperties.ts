import { SyntheticEvent } from 'react';

export interface ChildButtonProperties {
  className?: string;
  disabled?: boolean;
  iconFilled?: boolean;
  isLeft?: boolean;
  label?: string | null;
  onClick?: (e: SyntheticEvent) => void;
  style?: React.CSSProperties;
  tooltip?: string;
  outlined?: boolean;
}
