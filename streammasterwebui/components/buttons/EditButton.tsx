import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const EditButton: React.FC<ChildButtonProperties> = ({ disabled = false, iconFilled, label, onClick, tooltip = 'Edit' }) => (
  <SMButton
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
