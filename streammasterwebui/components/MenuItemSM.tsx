import '@lib/styles/MenuItemSM.css';
import { Tooltip } from 'primereact/tooltip';
import React from 'react';
import { MenuItem } from 'react-pro-sidebar';
import { Link, useLocation } from 'react-router-dom';
import { v4 as uuidv4 } from 'uuid';

const MenuItemSM = (props: MenuItemSMProperties) => {
  const tooltipClassName = React.useMemo(() => `menuitemsm-${uuidv4()}`, []);

  const location = useLocation();
  const { pathname } = location;

  if (props.collapsed) {
    return (
      <>
        <Tooltip target={`.${tooltipClassName}`} />
        <div
          className={`${tooltipClassName} border-white`}
          data-pr-hidedelay={100}
          data-pr-position="right"
          data-pr-showdelay={500}
          data-pr-tooltip={props.tooltip ?? props.name}
        >
          <MenuItem
            active={pathname === props.link}
            component={<Link className="link" target={props.newWindow === null ? '' : props.newWindow ? '_blank' : ''} to={props.link} />}
            icon={props.icon}
          >
            {props.children}
            {props.name}
          </MenuItem>
        </div>
      </>
    );
  }

  return (
    <MenuItem
      active={pathname === props.link}
      component={<Link className="link" target={props.newWindow === null ? '' : props.newWindow ? '_blank' : ''} to={props.link} />}
      icon={props.icon}
    >
      {props.children}
      {props.name}
    </MenuItem>
  );
};

export interface MenuItemSMProperties {
  readonly children?: React.ReactNode;
  readonly collapsed?: boolean;
  readonly icon: React.ReactNode;
  readonly link: string;
  readonly name: string;
  readonly newWindow?: boolean;
  readonly tooltip?: string;
}

export default React.memo(MenuItemSM);
