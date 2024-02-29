import BaseButton from './BaseButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const BookButton: React.FC<ChildButtonProperties> = ({ disabled = false, iconFilled = true, label, onClick, tooltip = '' }) => (
  <BaseButton disabled={disabled} icon="pi-book" iconFilled={iconFilled} label={label} onClick={onClick} tooltip={tooltip} />
);

export default BookButton;
