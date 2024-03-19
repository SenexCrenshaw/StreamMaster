import BaseButton from './BaseButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const MinusButton: React.FC<ChildButtonProperties> = ({ disabled = false, iconFilled, label, onClick, tooltip = 'Delete Stream' }) => (
  <BaseButton disabled={disabled} icon="pi-minus" iconFilled={false} onClick={onClick} severity="danger" tooltip={tooltip} />
);

export default MinusButton;
