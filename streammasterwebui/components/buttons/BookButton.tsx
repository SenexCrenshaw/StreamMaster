import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const BookButton: React.FC<ChildButtonProperties> = ({ buttonDisabled = false, iconFilled = true, label, onClick, tooltip = '' }) => (
  <SMButton buttonDisabled={buttonDisabled} icon="pi-book" iconFilled={iconFilled} label={label} onClick={onClick} tooltip={tooltip} />
);

export default BookButton;
