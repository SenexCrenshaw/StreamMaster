import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const BanButton: React.FC<ChildButtonProperties> = ({ buttonClassName, buttonDisabled = false, onClick, tooltip = '' }) => (
  <SMButton buttonClassName={buttonClassName} buttonDisabled={buttonDisabled} icon="pi-ban" iconFilled={false} onClick={onClick} tooltip={tooltip} />
);

export default BanButton;
