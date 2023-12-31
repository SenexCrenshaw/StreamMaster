import BaseButton, { type ChildButtonProperties } from './BaseButton';

const SaveButton: React.FC<ChildButtonProperties> = ({ className = '', disabled = false, iconFilled, label, onClick, tooltip = 'Add' }) => (
  <BaseButton
    className={className}
    disabled={disabled}
    icon="pi-save"
    iconFilled={iconFilled}
    label={label}
    onClick={onClick}
    severity="success"
    tooltip={tooltip}
  />
);

export default SaveButton;
