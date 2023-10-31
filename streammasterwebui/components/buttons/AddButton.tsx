import BaseButton, { type ChildButtonProps as ChildButtonProperties } from './BaseButton';

const AddButton: React.FC<ChildButtonProperties> = ({ disabled = false, iconFilled, label, onClick, tooltip = 'Add' }) => (
  <BaseButton
    disabled={disabled}
    icon="pi-plus"
    iconFilled={iconFilled}
    label={iconFilled === true ? undefined : label}
    onClick={onClick}
    severity="success"
    tooltip={tooltip}
  />
);

export default AddButton;
