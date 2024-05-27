import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const SaveButton: React.FC<ChildButtonProperties> = ({ className = 'icon-green-filled', disabled = false, iconFilled, label, onClick, tooltip = 'Add' }) => (
  <SMButton className={className} disabled={disabled} icon="pi-save" iconFilled={iconFilled} label={label} onClick={onClick} tooltip={tooltip} />
);

export default SaveButton;
