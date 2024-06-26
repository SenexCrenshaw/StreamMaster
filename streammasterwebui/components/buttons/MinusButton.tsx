import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const MinusButton: React.FC<ChildButtonProperties> = ({ buttonDisabled = false, iconFilled, label, onClick, tooltip = 'Delete Stream' }) => {
  return <SMButton buttonClassName="icon-red" buttonDisabled={buttonDisabled} icon="pi-minus" iconFilled={false} onClick={onClick} tooltip={tooltip} />;
};

export default MinusButton;
