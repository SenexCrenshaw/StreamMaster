import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const LeftArrowButton: React.FC<ChildButtonProperties> = ({ disabled = false, onClick, tooltip = 'Add' }) => (
  <SMButton disabled={disabled} icon="pi-chevron-left" iconFilled={false} onClick={onClick} severity="success" tooltip={tooltip} />
);

export default LeftArrowButton;
