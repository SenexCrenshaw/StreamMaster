import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const ResetButton: React.FC<ChildButtonProperties> = ({
  className = 'icon-yellow-filled',
  disabled = false,
  iconFilled,

  onClick,
  tooltip = 'Reset'
}) => <SMButton className={className} disabled={disabled} icon="pi-history" iconFilled={iconFilled} onClick={onClick} tooltip={tooltip} />;

export default ResetButton;
