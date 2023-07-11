/* eslint-disable react/no-unused-prop-types */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/consistent-type-imports */
import { MRT_ColumnDef, MaterialReactTable } from 'material-react-table';
import React, { useMemo } from "react";
import * as StreamMasterApi from '../store/iptvApi';
import * as Hub from '../store/signlar_functions';
import { Toast } from 'primereact/toast';

const SMDataTable = (props: SMDataTableProps) => {
  const toast = React.useRef<Toast>(null);

  type Person = {
    address: string;
    city: string;
    name: {
      firstName: string;
      lastName: string;
    };
    state: string;
  };

  const data: Person[] = [
    {
      address: '261 Erdman Ford',
      city: 'East Daphne',
      name: {
        firstName: 'John',
        lastName: 'Doe',
      },
      state: 'Kentucky',
    },
    {
      address: '769 Dominic Grove',
      city: 'Columbus',
      name: {
        firstName: 'Jane',
        lastName: 'Doe',
      },
      state: 'Ohio',
    },
    {
      address: '566 Brakus Inlet',
      city: 'South Linda',
      name: {
        firstName: 'Joe',
        lastName: 'Doe',
      },
      state: 'West Virginia',
    },
    {
      address: '722 Emie Stream',
      city: 'Lincoln',
      name: {
        firstName: 'Kevin',
        lastName: 'Vandy',
      },
      state: 'Nebraska',
    },
    {
      address: '32188 Larkin Turnpike',
      city: 'Omaha',
      name: {
        firstName: 'Joshua',
        lastName: 'Rolluffs',
      },
      state: 'Nebraska',
    },
  ];

  const columns = useMemo<Array<MRT_ColumnDef<Person>>>(
    () => [
      {
        accessorKey: 'name.firstName', // access nested data with dot notation
        header: 'First Name',
        size: 150,
      },
      {
        accessorKey: 'name.lastName',
        header: 'Last Name',
        size: 150,
      },
      {
        accessorKey: 'address', // normal accessorKey
        header: 'Address',
        size: 200,
      },
      {
        accessorKey: 'city',
        header: 'City',
        size: 150,
      },
      {
        accessorKey: 'state',
        header: 'State',
        size: 150,
      },
    ],
    [],
  );
  return <MaterialReactTable columns={columns} data={data} />;
}

SMDataTable.displayName = 'SMDataTable';
SMDataTable.defaultProps = {
  onChange: null,
  value: null,
};

type SMDataTableProps = {
  data?: StreamMasterApi.ChannelGroupDto | undefined;
  onChange?: ((value: string) => void) | null;
  value?: string | null;
};

export default React.memo(SMDataTable);
