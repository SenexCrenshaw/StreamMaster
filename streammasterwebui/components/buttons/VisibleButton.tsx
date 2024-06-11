import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const VisibleButton: React.FC<ChildButtonProperties> = ({
  buttonClassName = 'icon-blue',
  buttonDisabled = false,
  isLeft,
  iconFilled = true,
  label,
  onClick,
  tooltip = 'Toggle Visibility'
}) => (
  <SMButton
    buttonClassName={buttonClassName}
    buttonDisabled={buttonDisabled}
    icon="pi-eye-slash"
    iconFilled={iconFilled}
    isLeft={isLeft}
    label={iconFilled === true ? label || undefined : undefined}
    onClick={onClick}
    tooltip={tooltip}
  />
);

export default VisibleButton;
