import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const EditButton: React.FC<ChildButtonProperties> = ({ buttonDisabled = false, iconFilled, label, onClick, tooltip = 'Edit' }) => (
  <SMButton
    buttonDisabled={buttonDisabled}
    icon="pi-pencil"
    iconFilled={iconFilled}
    label={iconFilled === true ? undefined : label}
    onClick={onClick}
    tooltip={tooltip}
  />
);

export default EditButton;
