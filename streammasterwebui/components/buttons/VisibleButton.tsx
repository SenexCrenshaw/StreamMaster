import BaseButton from './BaseButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const VisibleButton: React.FC<ChildButtonProperties> = ({ disabled = false, iconFilled = true, label, onClick, tooltip = 'Toggle Visibility' }) => (
  <BaseButton
    disabled={disabled}
    icon="pi-eye-slash"
    iconFilled={iconFilled}
    label={iconFilled === true ? label || undefined : undefined}
    onClick={onClick}
    tooltip={tooltip}
  />
);

export default VisibleButton;
