import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const BanButton: React.FC<ChildButtonProperties> = ({ className, disabled = false, onClick, tooltip = '' }) => (
  <SMButton className={className} disabled={disabled} icon="pi-ban" iconFilled={false} onClick={onClick} tooltip={tooltip} />
);

export default BanButton;
