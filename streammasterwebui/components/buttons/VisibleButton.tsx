import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const VisibleButton: React.FC<ChildButtonProperties> = ({
  className,
  disabled = false,
  isLeft,
  iconFilled = true,
  label,
  onClick,
  tooltip = 'Toggle Visibility'
}) => (
  <SMButton
    className={className}
    disabled={disabled}
    icon="pi-eye-slash"
    iconFilled={iconFilled}
    isLeft={isLeft}
    label={iconFilled === true ? label || undefined : undefined}
    onClick={onClick}
    severity="danger"
    tooltip={tooltip}
  />
);

export default VisibleButton;
