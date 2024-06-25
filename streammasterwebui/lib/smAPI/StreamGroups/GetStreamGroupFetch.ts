import { GetStreamGroup } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { GetStreamGroupRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetStreamGroup = createAsyncThunk('cache/getGetStreamGroup', async (param: GetStreamGroupRequest, thunkAPI) => {
  try {
    Logger.debug('Fetching GetStreamGroup');
    const response = await GetStreamGroup(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


