import BaseButton from './BaseButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const ResetButton: React.FC<ChildButtonProperties> = ({ disabled = false, iconFilled = false, label, onClick, tooltip = '' }) => (
  <BaseButton disabled={disabled} icon="pi-history" iconFilled={iconFilled} label={label} onClick={onClick} tooltip={tooltip} />
);

export default ResetButton;
