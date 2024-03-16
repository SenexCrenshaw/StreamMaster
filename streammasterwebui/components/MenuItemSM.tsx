import { Tooltip } from 'primereact/tooltip';
import React from 'react';
import { MenuItem } from 'react-pro-sidebar';
import { Link, useLocation } from 'react-router-dom';
import { v4 as uuidv4 } from 'uuid';

export interface MenuItemSMProperties {
  readonly children?: React.ReactNode;
  readonly collapsed?: boolean;
  readonly icon: React.ReactNode;
  readonly link: string;
  readonly name: string;
  readonly newWindow?: boolean;
  readonly tooltip?: string;
}

const MenuItemSM = ({ children, collapsed, icon, link, name, newWindow, tooltip }: MenuItemSMProperties) => {
  const tooltipClassName = React.useMemo(() => `menuitemsm-${uuidv4()}`, []);

  const location = useLocation();
  const { pathname } = location;

  if (collapsed) {
    return (
      <>
        <Tooltip target={`.${tooltipClassName}`} />
        <div
          className={`${tooltipClassName} border-white`}
          data-pr-hidedelay={100}
          data-pr-position="right"
          data-pr-showdelay={500}
          data-pr-tooltip={tooltip ?? name}
        >
          <MenuItem
            active={pathname === link}
            component={<Link className="link" target={newWindow === null ? '' : newWindow ? '_blank' : ''} to={link} />}
            icon={icon}
          >
            {children}
            {name}
          </MenuItem>
        </div>
      </>
    );
  }

  return (
    <MenuItem
      active={pathname === link}
      component={<Link className="link" target={newWindow === null ? '' : newWindow ? '_blank' : ''} to={link} />}
      icon={icon}
    >
      {children}
      {name}
    </MenuItem>
  );
};

export default React.memo(MenuItemSM);
