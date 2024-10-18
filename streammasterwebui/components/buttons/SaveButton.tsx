import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const SaveButton: React.FC<ChildButtonProperties> = ({
  buttonClassName = 'icon-green-filled',
  buttonDisabled = false,
  iconFilled,
  label,
  onClick,
  tooltip = 'Add'
}) => (
  <SMButton
    buttonClassName={buttonClassName}
    buttonDisabled={buttonDisabled}
    icon="pi-save"
    iconFilled={iconFilled}
    label={label}
    onClick={onClick}
    tooltip={tooltip}
  />
);

export default SaveButton;
