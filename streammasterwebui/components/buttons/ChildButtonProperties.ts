import { SyntheticEvent } from 'react';

export interface ChildButtonProperties {
  className?: string;
  disabled?: boolean | undefined;
  iconFilled?: boolean;
  label?: string | undefined | null;
  onClick: (e: SyntheticEvent) => void;
  style?: React.CSSProperties | undefined;
  tooltip?: string;
}
