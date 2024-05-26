import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const SGRemoveButton: React.FC<ChildButtonProperties> = ({ onClick, disabled, tooltip }) => {
  return <SMButton disabled={disabled} className="icon-sg" icon="pi-check-circle" onClick={onClick} severity="secondary" tooltip={tooltip} rounded />;
};

export default SGRemoveButton;
