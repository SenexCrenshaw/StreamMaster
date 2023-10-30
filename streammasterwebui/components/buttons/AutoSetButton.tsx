import { type ChildButtonProps as ChildButtonProperties } from './BaseButton';
import BaseButton from './BaseButton';

const AutoSetButton: React.FC<ChildButtonProperties> = ({
  disabled = true,
  onClick,
  tooltip = 'Auto Set'
}) => (
  <BaseButton
    disabled={disabled}
    icon="pi-sort-numeric-up-alt"
    onClick={onClick}
    tooltip={tooltip}
  />
);

export default AutoSetButton;
