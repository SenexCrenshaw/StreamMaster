import { GetStreamGroup } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { GetStreamGroupRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetStreamGroup = createAsyncThunk('cache/getGetStreamGroup', async (param: GetStreamGroupRequest, thunkAPI) => {
  try {
    const response = await GetStreamGroup(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


