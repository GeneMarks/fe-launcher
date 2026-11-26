import { Injectable } from "@nestjs/common";
import { PrismaService } from "../prisma/prisma.service.js";
import { Process, Prisma } from "../../generated/prisma/client.js";


@Injectable()
export class ProcessService {
    constructor(private prisma: PrismaService) {}

    async process(processWhereUniqueInput: Prisma.ProcessWhereUniqueInput): Promise<Process | null> {
        return this.prisma.process.findUnique({
            where: processWhereUniqueInput,
        });
    }

    async processes(params: {
        skip?: number;
        take?: number;
        cursor?: Prisma.ProcessWhereUniqueInput;
        where?: Prisma.ProcessWhereInput;
        orderBy?: Prisma.ProcessOrderByWithRelationInput;
    }): Promise<Process[]> {
        const { skip, take, cursor, where, orderBy } = params;
        return this.prisma.process.findMany({
            skip,
            take,
            cursor,
            where,
            orderBy,
        });
    }

    async createProcess(data: Prisma.ProcessCreateInput): Promise<Process> {
        return this.prisma.process.create({
            data,
        });
    }

    async updateProcess(params: {
        where: Prisma.ProcessWhereUniqueInput;
        data: Prisma.ProcessUpdateInput;
    }): Promise<Process> {
        const { where, data } = params;
        return this.prisma.process.update({
            data,
            where,
        })
    }
}