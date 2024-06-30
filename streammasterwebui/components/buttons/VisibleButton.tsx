import { SMButtonProperties } from '@components/sm/Interfaces/SMButtonProperties';
import SMButton from '@components/sm/SMButton';
// import { ChildButtonProperties } from './ChildButtonProperties';

const VisibleButton: React.FC<SMButtonProperties> = ({
  buttonClassName = 'icon-blue',
  buttonDisabled = false,
  isLeft,
  menu,
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
    menu={menu}
    onClick={onClick}
    tooltip={tooltip}
  />
);

export default VisibleButton;
