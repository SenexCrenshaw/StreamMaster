import { getRightToolOptions } from '@lib/common/common';
import { Button } from 'primereact/button';
import { ChildButtonProperties } from './ChildButtonProperties';

interface SunButtonProperties extends ChildButtonProperties {
  isDark: boolean;
}

const SunButton: React.FC<SunButtonProperties> = ({ onClick, isDark }) => (
  <Button icon="pi pi-sun" text onClick={onClick} rounded tooltip={isDark === true ? 'Toggle Light' : 'Toggle Dark'} tooltipOptions={getRightToolOptions} />
);

export default SunButton;
