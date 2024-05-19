import SMButton from '@components/sm/SMButton';
import { Button } from 'primereact/button';
import React from 'react';
import { ChildButtonProperties } from './ChildButtonProperties';

const AddButton = React.forwardRef<Button, ChildButtonProperties>(({ disabled = false, iconFilled = true, label, onClick, tooltip = 'Add' }, ref) => (
  <SMButton
    // className={`p-1 px-2 text-xs` + iconFilled ? '' : 'w-2rem'}
    className="icon-green"
    disabled={disabled}
    icon="pi-plus"
    iconFilled={iconFilled}
    label={label ?? undefined}
    onClick={onClick}
    ref={ref}
    tooltip={tooltip}
  />
));

export default AddButton;
