import { type ChildButtonProps as ChildButtonProperties } from './BaseButton';
import BaseButton from './BaseButton';

const EditButton: React.FC<ChildButtonProperties> = ({ disabled = false, iconFilled, label, onClick, tooltip = 'Edit' }) => (
  <BaseButton
    disabled={disabled}
    icon="pi-pencil"
    iconFilled={iconFilled}
    label={iconFilled === true ? undefined : label}
    onClick={onClick}
    severity="warning"
    tooltip={tooltip}
  />
);

export default EditButton;
