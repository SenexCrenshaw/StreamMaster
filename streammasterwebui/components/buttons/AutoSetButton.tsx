import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const AutoSetButton: React.FC<ChildButtonProperties> = ({ disabled = true, onClick, tooltip = 'Auto Set' }) => (
  <SMButton disabled={disabled} icon="pi-sort-numeric-up-alt" onClick={onClick} tooltip={tooltip} />
);

export default AutoSetButton;
