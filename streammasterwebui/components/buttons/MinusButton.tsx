import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const MinusButton: React.FC<ChildButtonProperties> = ({ disabled = false, iconFilled, label, onClick, tooltip = 'Delete Stream' }) => {
  return <SMButton disabled={disabled} icon="pi-minus" iconFilled={false} onClick={onClick} severity="danger" tooltip={tooltip} />;
};

export default MinusButton;
