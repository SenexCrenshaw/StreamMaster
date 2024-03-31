import { SyntheticEvent } from 'react';

export interface ChildButtonProperties {
  className?: string;
  disabled?: boolean | undefined;
  iconFilled?: boolean;
  isLeft?: boolean;
  label?: string | undefined | null;
  onClick: (e: SyntheticEvent) => void;
  style?: React.CSSProperties | undefined;
  tooltip?: string;
  outlined?: boolean | undefined;
}
