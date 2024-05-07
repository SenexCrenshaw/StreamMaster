import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const ClearButton: React.FC<ChildButtonProperties> = ({ disabled = true, onClick, tooltip = '' }) => (
  <SMButton disabled={disabled} icon="pi-book" onClick={onClick} tooltip={tooltip} />
);

export default ClearButton;
