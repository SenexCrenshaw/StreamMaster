import { type ChildButtonProps as ChildButtonProperties } from './BaseButton';
import BaseButton from './BaseButton';

const ResetButton: React.FC<ChildButtonProperties> = ({
  disabled = false,
  onClick,
  tooltip = ''
}) => (
  <BaseButton
    disabled={disabled}
    icon="pi-history"
    iconFilled={false}
    onClick={onClick}
    tooltip={tooltip}
  />
);

export default ResetButton;
