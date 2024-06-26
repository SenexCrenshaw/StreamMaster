import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const ResetButton: React.FC<ChildButtonProperties> = ({
  buttonClassName = 'icon-yellow-filled',
  buttonDisabled = false,
  iconFilled,
  label,
  onClick,
  tooltip = 'Reset'
}) => (
  <SMButton
    buttonClassName={buttonClassName}
    buttonDisabled={buttonDisabled}
    icon="pi-history"
    iconFilled={iconFilled}
    label={label}
    onClick={onClick}
    tooltip={tooltip}
  />
);

export default ResetButton;
