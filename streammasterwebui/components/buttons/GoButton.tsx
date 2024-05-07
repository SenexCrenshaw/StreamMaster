import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const GoButton: React.FC<ChildButtonProperties> = ({ disabled = false, onClick, tooltip = 'Remove', iconFilled, label }) => (
  <SMButton disabled={disabled} icon="pi-times" iconFilled={iconFilled} label={label} onClick={onClick} severity="danger" tooltip={tooltip} />
);

export default GoButton;
