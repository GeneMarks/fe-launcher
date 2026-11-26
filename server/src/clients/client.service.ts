import { Injectable } from "@nestjs/common";
import { PrismaService } from "../prisma/prisma.service.js";
import { Client, Prisma } from "../../generated/prisma/client.js";


@Injectable()
export class ClientService {
    constructor(private prisma: PrismaService) {}

    async client(clientWhereUniqueInput: Prisma.ClientWhereUniqueInput): Promise<Client | null> {
        return this.prisma.client.findUnique({
            where: clientWhereUniqueInput,
        });
    }

    async clients(params: {
        skip?: number;
        take?: number;
        cursor?: Prisma.ClientWhereUniqueInput;
        where?: Prisma.ClientWhereInput;
        orderBy?: Prisma.ClientOrderByWithRelationInput;
    }): Promise<Client[]> {
        const { skip, take, cursor, where, orderBy } = params;
        return this.prisma.client.findMany({
            skip,
            take,
            cursor,
            where,
            orderBy,
        });
    }

    async createClient(data: Prisma.ClientCreateInput): Promise<Client> {
        return this.prisma.client.create({
            data,
        });
    }

    async updateClient(params: {
        where: Prisma.ClientWhereUniqueInput;
        data: Prisma.ClientUpdateInput;
    }): Promise<Client> {
        const { where, data } = params;
        return this.prisma.client.update({
            data,
            where,
        })
    }
}