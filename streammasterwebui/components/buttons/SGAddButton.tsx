import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const SGAddButton: React.FC<ChildButtonProperties> = ({ disabled, onClick, tooltip }) => {
  return <SMButton disabled={disabled} iconFilled={false} icon="pi-circle" onClick={onClick} severity="secondary" tooltip={tooltip} />;
};

export default SGAddButton;
