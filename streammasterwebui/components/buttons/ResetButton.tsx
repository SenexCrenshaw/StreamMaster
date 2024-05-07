import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const ResetButton: React.FC<ChildButtonProperties> = ({
  className = 'icon-yellow-filled',
  disabled = false,
  iconFilled,
  label = 'Reset',
  onClick,
  tooltip = 'Reset'
}) => (
  <SMButton
    className={className}
    disabled={disabled}
    icon="pi-history"
    iconFilled={iconFilled}
    label={iconFilled === true ? undefined : label ?? undefined}
    onClick={onClick}
    tooltip={tooltip}
  />
);

export default ResetButton;
