import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const SearchButton: React.FC<ChildButtonProperties> = ({ buttonDisabled = false, onClick, tooltip = 'Add' }) => (
  <SMButton buttonDisabled={buttonDisabled} icon="pi-search-plus" iconFilled={false} onClick={onClick} tooltip={tooltip} />
);

export default SearchButton;
